

# ECS Task Definition
resource "aws_ecs_task_definition" "this" {
  family                   = "${var.project}-${var.environment}-server"
  network_mode             = "awsvpc"
  requires_compatibilities = ["FARGATE"]
  cpu                      = var.cpu
  memory                   = var.memory
  execution_role_arn       = var.execution_role_arn
  task_role_arn            = aws_iam_role.ecs_task_role.arn

  container_definitions = jsonencode([
    {
      name      = var.service_name
      image     = var.image_uri
      essential = true
      portMappings = [
        {
          containerPort = var.container_port
          hostPort      = var.container_port
          protocol      = var.protocol
        }
      ]

      logConfiguration = {
        logDriver = "awslogs"
        options = {
          awslogs-group         = aws_cloudwatch_log_group.this.name
          awslogs-region        = var.region
          awslogs-stream-prefix = "ecs"
        }
      }

      readonlyRootFilesystem = var.readonlyRootFilesystem
    }
  ])
}

# ECS Service
resource "aws_ecs_service" "this" {
  name                   = "${var.project}-${var.environment}-server"
  cluster                = var.ecs_cluster_id
  task_definition        = aws_ecs_task_definition.this.arn
  desired_count          = var.desired_count
  launch_type            = "FARGATE"
  enable_execute_command = var.enable_execute_command

  network_configuration {

    subnets          = var.public_subnets_id
    security_groups  = [aws_security_group.ecs_service_sg.id]
    assign_public_ip = var.assign_public_ip
  }
}


# Security Group
resource "aws_security_group" "ecs_service_sg" {
  name        = "${var.project}-${var.environment}-ecs-service-sg"
  description = "Security group for ${var.project}-${var.environment} service"
  vpc_id      = var.vpc_id

  # Allow custom UDP port
  ingress {
    from_port   = var.container_port
    to_port     = var.container_port
    protocol    = "udp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  # Allow all outbound traffic
  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
}

# Cloudwatch Log Group
resource "aws_cloudwatch_log_group" "this" {
  name              = "/ecs/${var.project}-${var.environment}-server"
  retention_in_days = var.log_retention_days
}


# task role 
data "aws_iam_policy_document" "ecs_task_assume_role" {
  statement {
    actions = ["sts:AssumeRole"]
    principals {
      type        = "Service"
      identifiers = ["ecs-tasks.amazonaws.com"]
    }
  }
}

resource "aws_iam_role" "ecs_task_role" {
  name               = "${var.project}-${var.environment}-task-role"
  assume_role_policy = data.aws_iam_policy_document.ecs_task_assume_role.json
}


# If allow to exec into the container
data "aws_iam_policy_document" "ecs_exec" {
  count = var.enable_execute_command ? 1 : 0

  statement {
    actions = [
      "ssmmessages:CreateControlChannel",
      "ssmmessages:CreateDataChannel",
      "ssmmessages:OpenControlChannel",
      "ssmmessages:OpenDataChannel"
    ]
    resources = ["*"]
  }

  statement {
    actions   = ["logs:DescribeLogGroups"]
    resources = [aws_cloudwatch_log_group.this.arn]
  }

  statement {
    actions = [
      "logs:CreateLogStream",
      "logs:DescribeLogStreams",
      "logs:PutLogEvents"
    ]
    resources = ["${aws_cloudwatch_log_group.this.arn}:*"]
  }
}

resource "aws_iam_role_policy" "ecs_exec_policy" {
  count  = var.enable_execute_command ? 1 : 0
  name   = "${var.project}-${var.environment}-ecs-exec-policy"
  role   = var.execution_role_name
  policy = data.aws_iam_policy_document.ecs_exec[0].json
}
