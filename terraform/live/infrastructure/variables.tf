variable "core_config" {
  description = "Core configuration for the environment"
  type = object({
    project     = string
    environment = string
    region      = string
  })
}

variable "network_config" {
  description = "Network configuration parameters"
  type = object({
    vpc_cidr            = string
    public_subnet_cidrs = list(string)
  })
}

variable "nlb_config" {
  description = "Network Load Balancer and DNS configuration"
  type = object({
    domain_name       = string
    subdomain         = string
    health_check_port = number
  })
}