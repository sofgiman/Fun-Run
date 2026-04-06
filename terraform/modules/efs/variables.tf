variable "project" {
  type = string
}

variable "environment" {
  type = string
}

variable "vpc_id" {
  description = "The VPC ID where the EFS will live"
  type        = string
}

variable "vpc_cidr" {
  description = "The CIDR block of the VPC to allow internal traffic to EFS"
  type        = string
}

variable "subnets" {
  description = "List of subnet IDs to create mount targets in"
  type        = list(string)
}

variable "efs_id" {
  description = "The ID of the external EFS file system"
  type        = string
}
