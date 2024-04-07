package server;

import java.net.ServerSocket;
import java.net.Socket;
import java.util.concurrent.Executors;
import java.util.concurrent.ThreadPoolExecutor;

public class Server {

    private final static int N_THREADS = 4;

    public static void main(String[] args) {
        ThreadPoolExecutor threadPoolExecutor = (ThreadPoolExecutor) Executors.newFixedThreadPool(N_THREADS);
        try (ServerSocket serverSocket = new ServerSocket(4040)) {
            while (true) {
                Socket socket = serverSocket.accept();
                System.out.println("Client socket connected: " + socket.getPort());
                threadPoolExecutor.execute(new WorkerThread(socket));

            }
        } catch (Exception ex) {
            System.out.println(ex.getClass() + " " + ex.getMessage());
        }
    }
}