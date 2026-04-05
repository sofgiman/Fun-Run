variable "core_config" {
  description = "Core environment configuration"
  type = object({
    project         = string
    environment     = string
    region          = string
    tf_state_bucket = string
  })
}

variable "app_config" {
  description = "Application specific configurations (CPU, Memory, scaling)"
  type = object({
    service_name           = string
    desired_count          = number
    cpu                    = string
    memory                 = string
    readonlyRootFilesystem = bool
    efs_mount_path         = string
  })
}

variable "network_config" {
  description = "Network and routing configurations for the container"
  type = object({
    server_port               = number
    server_protocol           = string
    assign_public_ip          = bool
    health_check_grace_period = number
  })
}

# Dynamic variables usually injected by CI/CD
variable "image_uri" {
  description = "The image URI to deploy"
  type        = string
}

variable "git_commit_hash" {
  description = "The image tag hash"
  type        = string
  default     = ""
}

variable "desired_count_override" {
  description = "Override for the number of desired ECS tasks (used by CI/CD Stop Pipeline)"
  type        = number
  default     = null
}
