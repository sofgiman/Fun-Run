output "data_bundle" {
  description = "All data layer outputs bundled for easy consumption by the infra layer"
  value = {
    efs_id = aws_efs_file_system.this.id
  }
}
