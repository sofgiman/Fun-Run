data "terraform_remote_state" "infra" {
  backend = "s3"
  config = {
    bucket = var.tf_state_bucket
    key    = "infrastructure/${var.environment}/terraform.tfstate"
    region = var.region
  }
}


module "ecs_app" {
  source      = "../../modules/ecs_app"
  project     = var.project
  environment = var.environment
  region      = var.region

  vpc_id            = data.terraform_remote_state.infra.outputs.vpc_id
  public_subnets_id = data.terraform_remote_state.infra.outputs.public_subnets

  ecs_cluster_id      = data.terraform_remote_state.infra.outputs.ecs_cluster_id
  execution_role_arn  = data.terraform_remote_state.infra.outputs.ecs_execution_role_arn
  execution_role_name = data.terraform_remote_state.infra.outputs.ecs_execution_role_name

  image_uri       = var.image_uri
  git_commit_hash = var.git_commit_hash

  service_name           = var.service_name
  cpu                    = var.cpu
  memory                 = var.memory
  container_port         = var.server_port
  protocol               = var.server_protocol
  readonlyRootFilesystem = false
  desired_count          = var.desired_count
  enable_execute_command = true
  assign_public_ip       = var.assign_public_ip
  log_retention_days     = 30

}


