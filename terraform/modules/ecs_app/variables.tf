
variable "project" {
  description = "fun-run"
  type        = string
}

variable "environment" {
  description = "Environment name (dev, prod)"
  type        = string
  default     = "dev"
}

# Network Module
variable "vpc_id" {
  type = string
}

variable "public_subnets_id" {
  description = "List of subnet ids"
  type        = list(string)
}


# ECS Module
variable "ecs_cluster_id" {
  description = "The ID of the ECS cluster"
}

variable "execution_role_arn" {
  description = "ECS task arn"
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

# Container port to expose
variable "container_port" {
  type        = number
  default     = 7777
  description = "The port on which the container will listen."
}

variable "desired_count" {
  type        = number
  default     = 1
  description = "The number of ECS tasks to run."
}

# Whether to assign a public IP to the service
variable "assign_public_ip" {
  type        = bool
  default     = true
  description = "Whether to assign a public IP to the ECS service tasks."
}
