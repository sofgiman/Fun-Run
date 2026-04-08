terraform {
  required_version = ">= 1.0"
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.0"
    }
  }
}

provider "aws" {
  region = var.core_config.region
}

# The actual permanent EFS Drive
resource "aws_efs_file_system" "this" {
  creation_token   = "${var.core_config.project}-${var.core_config.environment}-efs"
  encrypted        = true
  performance_mode = "generalPurpose"

  tags = {
    Name = "${var.core_config.project}-${var.core_config.environment}-efs"
  }
}
