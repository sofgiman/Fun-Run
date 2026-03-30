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

  # Allow health check port
  ingress {
    from_port   = var.health_check_port
    to_port     = var.health_check_port
    protocol    = "tcp"
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