<?php
require_once 'class.phpmailer.php';
require_once 'class.smtp.php';
/**
 * Mail short summary.
 *
 * Mail description.
 *
 * @version 1.0
 * @author meshkov
 */
class Mailer
{
    public function Send( $to, $subject, $body)
    {
        $mail = new PHPMailer();
        $mail->isSMTP();      
        $mail->Host = 'localhost';
        $mail->From = 'no-reply@il2.info';
        $mail->FromName = 'IL2.Info';
        $mail->addAddress($to);
        $mail->Subject = $subject;
        $mail->Body    = $body;
        $mail->AltBody = $body;

        if(!$mail->send()) {
            echo 'Message could not be sent.';
            echo 'Mailer Error: ' . $mail->ErrorInfo;
        }   
    }
}
