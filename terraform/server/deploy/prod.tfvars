# prod.tfvars
project     = "fun-run"
environment = "prod"       
region      = "us-east-1"
tf_state_bucket = "fun-run-terraform-state"

service_name  = "server"
desired_count = 1         

cpu    = "256"  # 0.25 vCPU
memory = "512"  # 0.5 GB RAM

# network and mirror config
server_port      = 7777
server_protocol  = "udp"  
assign_public_ip = true   

# sqlite needs writable file system
readonlyRootFilesystem = false