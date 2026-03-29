output "vpc_id" { value = module.network.vpc_id }
output "public_subnets" { value = module.network.public_subnets }
output "internet_gateway_id" { value = module.network.internet_gateway_id }

output "ecs_cluster_id" { value = module.ecs_infra.ecs_cluster_id }
output "ecs_execution_role_arn" { value = module.ecs_infra.ecs_execution_role_arn }
output "ecs_execution_role_name" { value = module.ecs_infra.ecs_execution_role_name }

output "target_group_arn" { value = module.nlb.target_group_arn }
output "nlb_dns_name" { value = module.nlb.nlb_dns_name }
output "health_check_port" { value = module.nlb.health_check_port }

output "infra_bundle" {
  description = "All infrastructure outputs bundled into a single object for easy consumption by the server layer"
  value = {
    vpc_id                  = module.network.vpc_id
    public_subnets          = module.network.public_subnets
    ecs_cluster_id          = module.ecs_infra.ecs_cluster_id
    ecs_execution_role_arn  = module.ecs_infra.ecs_execution_role_arn
    ecs_execution_role_name = module.ecs_infra.ecs_execution_role_name
    target_group_arn        = module.nlb.target_group_arn
    health_check_port       = module.nlb.health_check_port
    game_url                = var.nlb_config.domain_name != "" ? "${var.nlb_config.subdomain}.${var.nlb_config.domain_name}" : module.nlb.nlb_dns_name
  }
}