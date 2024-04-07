package server;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.Socket;

import util.ConnectionConstants;

public class WorkerThread extends Thread {

    private Socket socket;
    private BufferedReader bufferedReader;
    private PrintWriter printWriter;

    public WorkerThread(Socket s) throws IOException {
        super();
        socket = s;
        bufferedReader = new BufferedReader(new InputStreamReader(socket.getInputStream()));
        printWriter = new PrintWriter(socket.getOutputStream(), true);
    }

    @Override
    public void run() {
        try {
            String clientMsg = "";
            do {
                // Read client message
                clientMsg = bufferedReader.readLine();

                // Fetch the API for the client word
                ResponseEntity response = DictionaryAPI.read(clientMsg);

                // Write the response back to the user
                printWriter.println(response.getCode() + "");
                printWriter.println(response.getResponse());

            } while (!clientMsg.equals(ConnectionConstants.EXIT));

            bufferedReader.close();
            printWriter.close();

            System.out.println("Client socket closed: " + socket.getPort());
        } catch (IOException e) {
            e.printStackTrace();
        }

    }
}
