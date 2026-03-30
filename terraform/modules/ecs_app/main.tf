# Cloudwatch Log Group
resource "aws_cloudwatch_log_group" "this" {
  name              = "/ecs/${var.project}-${var.environment}-server"
  retention_in_days = var.log_retention_days
}

# ECS Task Definition
resource "aws_ecs_task_definition" "this" {
  family                   = "${var.project}-${var.environment}-server"
  network_mode             = "awsvpc"
  requires_compatibilities = ["FARGATE"]
  cpu                      = var.cpu
  memory                   = var.memory
  execution_role_arn       = var.execution_role_arn
  task_role_arn            = aws_iam_role.ecs_task_role.arn

  volume {
    name = "efs-storage"
    efs_volume_configuration {
      file_system_id     = var.efs_id
      transit_encryption = "ENABLED"
    }
  }
  
  container_definitions = jsonencode([
    {
      name      = var.service_name
      image     = var.image_uri
      essential = true

      # Set the environment variable to point to the EFS mount point
      environment = [
        {
          name  = "FUN_RUN_DATA_PATH"
          value = var.efs_mount_path
        }
      ]

      # Mount the EFS volume to the requested path
      mountPoints = [
        {
          sourceVolume  = "efs-storage"
          containerPath = var.efs_mount_path
          readOnly      = false
        }
      ]

      # Mount the EFS volume to /app/data
      mountPoints = [
        {
          sourceVolume  = "efs-storage"
          containerPath = "/app/data"  # <--- This is the path inside the container
          readOnly      = false
        }
      ]

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

      dockerLabels = {
        "git_commit_hash" = var.git_commit_hash
      }

      readonlyRootFilesystem = var.readonlyRootFilesystem
    },
    {
      name      = "health-check-sidecar"
      image     = "nginx:alpine"
      essential = true
      portMappings = [
        {
          containerPort = var.health_check_port
          hostPort      = var.health_check_port
          protocol      = "tcp"
        }
      ]
    }
  ])
}

# ECS Service
resource "aws_ecs_service" "this" {
  name                              = "${var.project}-${var.environment}-server"
  cluster                           = var.ecs_cluster_id
  task_definition                   = aws_ecs_task_definition.this.arn
  desired_count                     = var.desired_count
  launch_type                       = "FARGATE"
  enable_execute_command            = var.enable_execute_command
  health_check_grace_period_seconds = var.health_check_grace_period
  deployment_minimum_healthy_percent = 0
  deployment_maximum_percent         = 100

  load_balancer {
    target_group_arn = var.target_group_arn
    container_name   = var.service_name
    container_port   = var.container_port
  }

  network_configuration {
    subnets          = var.public_subnets_id
    security_groups  = [aws_security_group.ecs_service_sg.id]
    assign_public_ip = var.assign_public_ip
  }
}