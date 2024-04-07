// package client;

// import java.io.DataInputStream;
// import java.io.DataOutputStream;
// import java.net.Socket;
// import java.util.Scanner;

// public class Client {
// static Scanner scanner = new Scanner(System.in);

// public static void main(String[] args) {
// DataOutputStream outputStream;
// DataInputStream inputStream;

// try (Socket socket = new Socket("localhost", 4040)) {
// // input/output streams for exchanging data
// outputStream = new DataOutputStream(socket.getOutputStream());
// inputStream = new DataInputStream(socket.getInputStream());
// while (true) {
// String word = scanner.nextLine();
// System.out.println("Word: " + word);
// outputStream.writeUTF(word);
// System.out.println(inputStream.readUTF());
// }

// } catch (Exception ex) {
// System.out.println(ex.getMessage());
// }
// }
// }
