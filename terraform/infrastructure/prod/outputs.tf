output "vpc_id" {
  description = "The ID of the VPC"
  value       = module.network.vpc_id
}

output "public_subnets" {
  description = "List of public subnet IDs"
  value       = module.network.public_subnets
}

output "internet_gateway_id" {
  description = "The ID of the internet gateway"
  value       = module.network.internet_gateway_id
}

output "ecs_cluster_id" {
  description = "ECS cluster ID"
  value       = module.ecs_infra.ecs_cluster_id
}

output "ecs_execution_role_arn" {
  description = "ECS task arn"
  value       = module.ecs_infra.ecs_execution_role_arn
}

output "ecs_execution_role_name" {
  description = "ECS task arn"
  value       = module.ecs_infra.ecs_execution_role_name
}
