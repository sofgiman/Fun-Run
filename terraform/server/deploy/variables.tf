variable "project" {
  description = "fun-run"
  type        = string
  default     = "fun-run"
}

variable "environment" {
  description = "Environment name (dev, prod)"
  type        = string
  default     = "dev"
}

variable "region" {
  description = "AWS region"
  type        = string
  default     = "us-east-1"
}

variable "tf_state_bucket" {
  description = "The S3 bucket name where the infrastructure state is stored"
  type        = string
  default     = "fun-run-terraform-state"
}

# Server Container Specs

# Name of the ECS service
variable "service_name" {
  description = "The name of the ECS service."
  type        = string
  default     = "server"
}

variable "image_uri" {
  description = "The image URI to deploy"
  type        = string
}

# To allow redeploy of a task when using same image 
variable "git_commit_hash" {
  description = "The image tag hash"
  type        = string
  default     = ""
}


variable "cpu" {
  type    = string
  default = "256"
}

variable "memory" {
  type    = string
  default = "512"
}

# Unity mirror default port is 7777
variable "server_port" {
  type    = number
  default = 7777
}

# Container port to expose
variable "server_protocol" {
  type    = string
  default = "udp"
}

# Needed to be true when sqlite is on file system
# false when mounting efs for sqlite
variable "readonlyRootFilesystem" {
  type    = bool
  default = true
}

variable "desired_count" {
  type    = number
  default = 1
}

# Whether to assign a public IP to the service
variable "assign_public_ip" {
  type    = bool
  default = true
}

variable "health_check_grace_period" {
  type    = number
  default = 60
}