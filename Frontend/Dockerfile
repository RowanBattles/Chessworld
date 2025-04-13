# Use a Node.js image to build and serve the frontend
FROM node:20-alpine

# Set the working directory
WORKDIR /app

# Copy package.json and install dependencies
COPY package.json yarn.lock ./
RUN yarn install

# Copy the rest of the application code
COPY . .

# Inject the VITE_API_URL environment variable at build time with a default value
ARG VITE_API_URL
ENV VITE_API_URL=${VITE_API_URL}

# Build the application
RUN VITE_API_URL=${VITE_API_URL} yarn build

# Build the application
RUN yarn build

# Expose port 3000
EXPOSE 3000

# Start the application using a lightweight HTTP server
CMD ["npx", "serve", "-s", "dist", "-l", "3000"]