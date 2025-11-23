data "terraform_remote_state" "infra" {
  backend = "s3"
  config = {
    bucket = "fun-run-terraform-state"
    key    = "dev/terraform.tfstate"
    region = "us-east-1"
  }
}


module "ecs_app" {
  source  = "../../modules/ecs_app"
  project = var.project

  vpc_id            = data.terraform_remote_state.infra.outputs.vpc_id
  public_subnets_id = data.terraform_remote_state.infra.outputs.public_subnets

  ecs_cluster_id     = data.terraform_remote_state.infra.outputs.ecs_cluster_id
  execution_role_arn = data.terraform_remote_state.infra.outputs.ecs_execution_role_arn

  service_name     = var.service_name
  image_uri        = var.image_uri
  cpu              = var.cpu
  memory           = var.memory
  container_port   = var.server_port
  desired_count    = var.desired_count
  assign_public_ip = var.assign_public_ip
}

