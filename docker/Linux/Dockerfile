# Use a minimal runtime base image
FROM ubuntu:22.04


# Install dependencies required for Unity headless builds
RUN apt-get update && apt-get install -y libsqlite3-0 && rm -rf /var/lib/apt/lists/*

# Copy the Unity build into the container
WORKDIR /server
COPY . .

RUN chmod +x "fun-run-server"

# Expose the 7777 mirror port using udp protocol
EXPOSE 7777/udp

# Set the default entrypoint
ENTRYPOINT ["./fun-run-server", "-batchmode", "-nographics", "-logfile", "/dev/stdout"]
