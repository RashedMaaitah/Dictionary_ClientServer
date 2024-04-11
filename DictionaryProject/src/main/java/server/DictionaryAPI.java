package server;

import org.json.JSONArray;
import org.json.JSONObject;

import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;

class DictionaryAPI {

    static ResponseEntity read(String word) {

        String response = "";
        try {
            URL url = new URL(String.format("https://api.dictionaryapi.dev/api/v2/entries/en/%s", word));
            HttpURLConnection connection = (HttpURLConnection) url.openConnection();
            connection.setRequestMethod("GET");

            int responseCode = connection.getResponseCode();

            if (responseCode != 200)
                return new ResponseEntity("", responseCode);

            BufferedReader reader = new BufferedReader(new InputStreamReader(connection.getInputStream()));
            StringBuilder jsonString = new StringBuilder();
            String line = "";
            while ((line = reader.readLine()) != null) {
                jsonString.append(line);
            }
            reader.close();

            JSONArray jsonArray = new JSONArray(jsonString.toString());

            // Extract the json object
            JSONObject jsonObject = jsonArray.getJSONObject(0);

            // Extract the 'meanings' json array
            JSONArray meaningsArray = jsonObject.getJSONArray("meanings");

            for (int i = 0; i < meaningsArray.length(); i++) {

                // Extract a 'meaning' object from the 'meanings' array
                JSONObject obj = meaningsArray.getJSONObject(i);

                // Add the meaning part of speach e.g noun, verb or adjective
                response += String.format("As %s:", obj.getString("partOfSpeech"));

                // Extract the definitions array for the word meaning
                JSONArray defArr = obj.getJSONArray("definitions");

                // Extract the definition from the definitions and concatnate it to the response
                for (int j = 0; j < defArr.length(); j++) {
                    String definition = defArr.getJSONObject(j).getString("definition");
                    response += definition + "~";
                }
            }
            // Close the http connection with the API server
            connection.disconnect();

            return new ResponseEntity(response, responseCode);

        } catch (Exception ex) {
            System.out.println(ex.getClass() + " " + ex.getMessage());
            return new ResponseEntity("", 500);
        }
    }

}