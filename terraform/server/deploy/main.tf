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

# Pulling the state from the infrastructure layer
data "terraform_remote_state" "infra" {
  backend = "s3"
  config = {
    bucket = var.core_config.tf_state_bucket
    key    = "infrastructure/${var.core_config.environment}/terraform.tfstate"
    region = var.core_config.region
  }
}

locals {
  infra = data.terraform_remote_state.infra.outputs.infra_bundle
}

module "ecs_app" {
  source = "../../modules/ecs_app"

  # Core
  project     = var.core_config.project
  environment = var.core_config.environment
  region      = var.core_config.region

  # Infrastructure dependencies (Cleanly mapped via local.infra)
  vpc_id              = local.infra.vpc_id
  public_subnets_id   = local.infra.public_subnets
  ecs_cluster_id      = local.infra.ecs_cluster_id
  execution_role_arn  = local.infra.ecs_execution_role_arn
  execution_role_name = local.infra.ecs_execution_role_name
  target_group_arn    = local.infra.target_group_arn
  health_check_port   = local.infra.health_check_port
  efs_id              = local.infra.efs_id

  # App Specs
  service_name           = var.app_config.service_name
  cpu                    = var.app_config.cpu
  memory                 = var.app_config.memory
  readonlyRootFilesystem = var.app_config.readonlyRootFilesystem
  # Use CLI override if provided, otherwise fallback to app_config tfvars
  desired_count  = var.desired_count_override != null ? var.desired_count_override : var.app_config.desired_count
  efs_mount_path = var.app_config.efs_mount_path

  # CI/CD Injected
  image_uri       = var.image_uri
  git_commit_hash = var.git_commit_hash

  # Network
  container_port            = var.network_config.server_port
  protocol                  = var.network_config.server_protocol
  assign_public_ip          = var.network_config.assign_public_ip
  health_check_grace_period = var.network_config.health_check_grace_period

  # Defaults/Hardcoded
  enable_execute_command = true
  log_retention_days     = 30
}
