# core
variable "project" {
  type = string
}

variable "environment" {
  type    = string
  default = "dev"
}

variable "region" {
  type    = string
  default = "us-east-1"
}

# main.tf (ECS & Task)
variable "ecs_cluster_id" {
  type = string
}

variable "execution_role_arn" {
  type = string
}

variable "execution_role_name" {
  type = string
}

variable "service_name" {
  type    = string
  default = "server"
}

variable "image_uri" {
  type = string
}

variable "git_commit_hash" {
  type    = string
  default = ""
}

variable "cpu" {
  type    = string
  default = "256"
}

variable "memory" {
  type    = string
  default = "512"
}

variable "container_port" {
  type    = number
  default = 7777
}

variable "protocol" {
  type    = string
  default = "udp"
}

variable "readonlyRootFilesystem" {
  type    = bool
  default = true
}

variable "desired_count" {
  type    = number
  default = 1
}

variable "health_check_port" {
  type    = number
  default = 80
}

variable "health_check_grace_period" {
  type    = number
  default = 60
}

variable "log_retention_days" {
  type    = number
  default = 30
}

variable "target_group_arn" {
  type = string
}

variable "efs_id" {
  description = "The ID of the EFS file system for persistent storage"
  type        = string
}

# sg.tf (Networking)
variable "vpc_id" {
  type = string
}

variable "public_subnets_id" {
  type = list(string)
}

variable "assign_public_ip" {
  type    = bool
  default = true
}

# iam.tf (Permissions)
variable "enable_execute_command" {
  type    = bool
  default = false
}