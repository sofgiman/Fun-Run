
variable "project" {
  description = "fun-run"
  type        = string
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

variable "execution_role_name" {
  description = "ECS task name"
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

# Container port to expose
variable "container_port" {
  type        = number
  default     = 7777
  description = "The port on which the container will listen."
}

# Container port to expose
variable "protocol" {
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
  type        = number
  default     = 1
  description = "The number of ECS tasks to run."
}

# Allow to exec into the container
variable "enable_execute_command" {
  type    = bool
  default = false
}


# Whether to assign a public IP to the service
variable "assign_public_ip" {
  type        = bool
  default     = true
  description = "Whether to assign a public IP to the ECS service tasks."
}

variable "log_retention_days" {
  type    = number
  default = 30
}

