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

# Pulling the state from the Data layer
data "terraform_remote_state" "data_layer" {
  backend = "s3"
  config = {
    bucket = var.core_config.tf_state_bucket
    key    = "data/${var.core_config.environment}/terraform.tfstate"
    region = var.core_config.region
  }
}

locals {
  data = data.terraform_remote_state.data_layer.outputs.data_bundle
}

module "network" {
  source              = "../../modules/network"
  project             = var.core_config.project
  environment         = var.core_config.environment
  region              = var.core_config.region
  vpc_cidr            = var.network_config.vpc_cidr
  public_subnet_cidrs = var.network_config.public_subnet_cidrs
}

module "efs" {
  source      = "../../modules/efs"
  project     = var.core_config.project
  environment = var.core_config.environment

  vpc_id   = module.network.vpc_id
  vpc_cidr = var.network_config.vpc_cidr
  subnets  = module.network.public_subnets

  efs_id = local.data.efs_id
}

module "ecs_infra" {
  source      = "../../modules/ecs_infra"
  project     = var.core_config.project
  environment = var.core_config.environment
  region      = var.core_config.region
}

module "nlb" {
  source      = "../../modules/nlb"
  project     = var.core_config.project
  environment = var.core_config.environment
  region      = var.core_config.region

  vpc_id         = module.network.vpc_id
  public_subnets = module.network.public_subnets

  domain_name       = var.nlb_config.domain_name
  subdomain         = var.nlb_config.subdomain
  health_check_port = var.nlb_config.health_check_port
}
