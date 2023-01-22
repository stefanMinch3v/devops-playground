import http from 'k6/http';

export const options = {
  vus: 50,
  duration: '1m',
  ext: {
    loadimpact: {
      name: "webapp test"
    }
  }
};

export default function () {
  const url = 'http://35.202.234.153:5010'
  const res = http.get(url);
  
  if (res.status != 200) {
      console.log(res.body);
  }
}