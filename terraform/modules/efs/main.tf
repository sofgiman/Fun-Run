# Security Group for EFS
resource "aws_security_group" "efs_sg" {
  name        = "${var.project}-${var.environment}-efs-sg"
  description = "Allow NFS traffic into EFS from the VPC"
  vpc_id      = var.vpc_id

  ingress {
    description = "Allow NFS (2049) from VPC"
    from_port   = 2049
    to_port     = 2049
    protocol    = "tcp"
    cidr_blocks = [var.vpc_cidr]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
}

# Mount Targets (The "network plugs" inside your subnets)
resource "aws_efs_mount_target" "this" {
  count           = length(var.subnets)
  file_system_id  = var.efs_id
  subnet_id       = var.subnets[count.index]
  security_groups = [aws_security_group.efs_sg.id]
}
