output "target_group_arn" {
  description = "ARN of the Target Group to connect the ECS Service to"
  value       = aws_lb_target_group.udp_7777.arn
}

output "nlb_dns_name" {
  description = "The DNS name of the Load Balancer"
  value       = aws_lb.this.dns_name
}