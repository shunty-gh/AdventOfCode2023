# There's a bug in the python devcontainer feature when using the default 'bookworm' variant
# of the dotnet dockerfile. Therefore we'll specify the 'jammy' variant for now.
#FROM mcr.microsoft.com/devcontainers/dotnet:1-8.0
FROM mcr.microsoft.com/devcontainers/dotnet:1-8.0-jammy

COPY ./.bash_aliases /root/.bash_aliases
COPY ./.bash_aliases $HOME/.bash_aliases

## General dev tools
RUN apt-get update && apt-get install -y --no-install-recommends \
  curl \
  mc \
  htop \
  nano \
  ripgrep \
  fd-find
  #python3 \
  #pipx

RUN apt-get upgrade -y \
  && dotnet workload update

## Setup other software - all done in the devcontainer.json file
# If using a tool that doesn't support devcontainers then you can use the devcontainer CLI
# at https://github.com/devcontainers/cli
# Install it using
#    $> npm install -g @devcontainers/cli
# From the project root directory run
#    $> devcontainer up --workspace-folder .
