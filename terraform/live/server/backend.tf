terraform {
  backend "s3" {
    bucket         = "fun-run-terraform-state"
    region         = "us-east-1"
    dynamodb_table = "dynamoDB_to_lock_terraform_state"
    encrypt        = true
  }
}