package server;

import java.net.ServerSocket;
import java.net.Socket;
import java.util.concurrent.Executors;
import java.util.concurrent.ThreadPoolExecutor;

public class Server {

    public static void main(String[] args) {
        int N_THREADS = Integer.parseInt(args[0]);
        int portNumber = Integer.parseInt(args[1]);

        ThreadPoolExecutor threadPoolExecutor = (ThreadPoolExecutor) Executors.newFixedThreadPool(N_THREADS);
        try (ServerSocket serverSocket = new ServerSocket(portNumber)) {
            System.out.println("Server launched at port " + portNumber);
            while (true) {
                Socket socket = serverSocket.accept();
                System.out.println("Client socket connected: " + socket.getPort());
                threadPoolExecutor.execute(new WorkerThread(socket));

            }
        } catch (Exception ex) {
            System.out.println(ex.getClass() + " " + ex.getMessage());
        } finally {
            threadPoolExecutor.shutdown();
        }
    }
}