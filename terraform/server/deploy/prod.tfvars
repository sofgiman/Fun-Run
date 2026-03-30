# prod.tfvars
core_config = {
  project         = "fun-run"
  environment     = "prod"
  region          = "us-east-1"
  tf_state_bucket = "fun-run-terraform-state"
}

app_config = {
  service_name           = "server"
  desired_count          = 1
  cpu                    = "256"
  memory                 = "512"
  readonlyRootFilesystem = true # false if SQLite is local then it needs writable file system
}

network_config = {
  server_port               = 7777
  server_protocol           = "udp"
  assign_public_ip          = true
  health_check_grace_period = 60
}