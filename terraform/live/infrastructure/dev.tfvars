# dev.tfvars
core_config = {
  project     = "fun-run"
  environment = "dev"
  region      = "us-east-1"
  tf_state_bucket = "fun-run-terraform-state"
}

network_config = {
  vpc_cidr            = "10.0.0.0/16"
  public_subnet_cidrs = [
    "10.0.0.0/24"
  ]
}

nlb_config = {
  domain_name       = "idanyafe.com"
  subdomain         = "fun-run-dev"
  health_check_port = 80
}