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
  region = "us-east-1"
}

module "network" {
  source              = "../../modules/network"
  project             = var.project
  environment         = var.environment
  region              = var.region
  vpc_cidr            = var.vpc_cidr
  public_subnet_cidrs = var.public_subnet_cidrs

}

module "ecs_infra" {
  source      = "../../modules/ecs_infra"
  project     = var.project
  environment = var.environment
  region      = var.region

}

module "nlb" {
  source         = "../../modules/nlb"
  project        = var.project
  environment    = var.environment
  region         = var.region
  
  vpc_id         = module.network.vpc_id
  public_subnets = module.network.public_subnets
  
  # Leave empty if don't have a domain name yet, it will create NLB without domain name
  domain_name    = "" 
  subdomain      = var.subdomain

  health_check_port         = var.health_check_port
  health_check_grace_period = var.health_check_grace_period
}
