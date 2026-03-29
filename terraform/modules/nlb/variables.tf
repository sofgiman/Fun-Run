variable "project" { type = string }
variable "environment" { type = string }
variable "region" { type = string }
variable "vpc_id" { type = string }
variable "public_subnets" { type = list(string) }
variable "game_port" { 
  type = number 
  default = 7777
}
variable "domain_name" {
  description = "The root domain name (e.g., mygame.com). Leave empty if you don't have one yet."
  type = string
  default = ""
}

variable "subdomain" {
  description = "The subdomain prefix (e.g., 'play', 'fun-run', or 'api-dev')"
  type        = string
  default     = ""
}