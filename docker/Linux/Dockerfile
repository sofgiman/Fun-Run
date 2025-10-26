# Use a minimal runtime base image
FROM ubuntu:22.04


# Install dependencies required for Unity headless builds
RUN apt-get update && apt-get install -y libsqlite3-0 && rm -rf /var/lib/apt/lists/*

# Copy the Unity build into the container
WORKDIR /server
COPY Fun-Run-Server-ubuntu/ .

# Make the binary executable (sometimes lost in artifact)
RUN chmod +x "Fun Run Server"

# Expose the port your
EXPOSE 7777

# Set the default entrypoint
ENTRYPOINT ["./Fun Run Server", "-batchmode", "-nographics", "-logfile", "/dev/stdout"]
