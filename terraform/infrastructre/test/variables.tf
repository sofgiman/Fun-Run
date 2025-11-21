variable "project" {
  description = "fun-run"
  type        = string
}

variable "environment" {
  description = "Environment name (test, prod)"
  type        = string
  default     = "test"
}

variable "region" {
  description = "AWS region"
  type        = string
  default     = "us-east-1"
}

variable "vpc_cidr" {
  type    = string
  default = "10.0.0.0/16"
}

