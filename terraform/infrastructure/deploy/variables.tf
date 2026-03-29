variable "project" {
  description = "fun-run"
  type        = string
  default     = "fun-run"
}

variable "environment" {
  description = "Environment name (dev, prod)"
  type        = string
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

variable "public_subnet_cidrs" {
  description = "List of public subnet CIDRs"
  type        = list(string)
  default     = ["10.0.0.0/24", "10.0.1.0/24"]
}

variable "enable_execute_command" {
  type    = bool
  default = false
}

variable "domain_name" {
  description = "The root domain name (e.g., mygame.com). Leave empty if you don't have one yet."
  type        = string
  default     = ""
}

variable "subdomain" {
  description = "The subdomain for the environment (e.g., play, funrun-dev)"
  type        = string
  default     = ""
}

variable "health_check_port" {
  type    = number
  default = 80
}

variable "health_check_grace_period" {
  type    = number
  default = 60
}