# dev.tfvars
core_config = {
  project     = "fun-run"
  environment = "dev"
  region      = "us-east-1"
}

network_config = {
  vpc_cidr            = "10.0.0.0/16"
  public_subnet_cidrs = [
    "10.0.0.0/24", 
    "10.0.1.0/24"
  ]
}

nlb_config = {
  domain_name       = "idanyafe.com"
  subdomain         = "fun-run-dev"
  health_check_port = 80
}