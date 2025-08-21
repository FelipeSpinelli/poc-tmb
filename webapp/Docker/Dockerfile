FROM node:20 AS build
WORKDIR /app

COPY package*.json ./

RUN npm install

COPY . .

ARG API_BASE_URL
ENV VITE_API_BASE_URL=${API_BASE_URL}

RUN echo $VITE_API_BASE_URL

RUN npm run build

FROM nginx:stable-alpine
WORKDIR /usr/share/nginx/html

RUN rm -rf ./*

COPY --from=build /app/dist .

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
