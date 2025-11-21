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
  public_subnet_cidrs = ["10.0.0.0/24", "10.0.1.0/24"]

}

