# prod.tfvars
core_config = {
  project     = "fun-run"
  environment = "prod"
  region      = "us-east-1"
  tf_state_bucket = "fun-run-terraform-state"
}

network_config = {
  vpc_cidr            = "10.1.0.0/16"
  public_subnet_cidrs = [
    "10.1.0.0/24"
  ]
}

nlb_config = {
  domain_name       = "idanyafe.com"
  subdomain         = "fun-run"
  health_check_port = 80
}