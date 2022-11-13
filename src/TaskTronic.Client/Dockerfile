FROM node:12.13.1
 
WORKDIR /app
ENV PATH /app/node_modules/.bin:$PATH
 
COPY package.json /app/package.json
RUN npm install
RUN npm install -g @angular/cli@8.3.26
 
COPY . /app
 
CMD ng serve --host 0.0.0.0