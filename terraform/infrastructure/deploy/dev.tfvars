# dev.tfvars
environment = "dev"      
project     = "fun-run"  
region      = "us-east-1"

# network
vpc_cidr = "10.0.0.0/16"
public_subnet_cidrs = [
  "10.0.0.0/24",
  "10.0.1.0/24"
]

enable_execute_command = true

domain_name = "idanyafe.com"
subdomain = "fun-run-dev"  