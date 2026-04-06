variable "core_config" {
  description = "Core configuration for the environment"
  type = object({
    project     = string
    environment = string
    region      = string
  })
}
