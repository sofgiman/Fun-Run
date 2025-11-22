

# ECS Task Definition
resource "aws_ecs_task_definition" "this" {
  family                   = "${var.project}-server"
  network_mode             = "awsvpc"
  requires_compatibilities = ["FARGATE"]
  cpu                      = var.cpu
  memory                   = var.memory
  execution_role_arn       = var.execution_role_arn

  container_definitions = jsonencode([
    {
      name      = var.service_name
      image     = var.image_uri
      essential = true
      portMappings = [
        {
          containerPort = var.container_port
          hostPort      = var.container_port
        }
      ]
    }
  ])
}

# ECS Service
resource "aws_ecs_service" "this" {
  name            = "${var.project}-server"
  cluster         = var.ecs_cluster_id
  task_definition = aws_ecs_task_definition.this.arn
  desired_count   = var.desired_count
  launch_type     = "FARGATE"

  network_configuration {

    subnets          = var.public_subnets_id
    security_groups  = [aws_security_group.ecs_service_sg]
    assign_public_ip = var.assign_public_ip
  }
}

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
