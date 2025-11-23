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
