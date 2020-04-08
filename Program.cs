using System;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using MailKit.Net.Pop3;
using MailKit.Net.Imap;
using System.Linq;
using System.Collections.Generic;
using MailKit.Search;
using System.Text.RegularExpressions;

namespace EmailOptions
{
	class ReceiveMail
	{
		public string messageId { get; set; }
		public string messageFrom { get; set; }
		public string messageSubject { get; set; }
	}
	class Program
	{
		static void Main(string[] args)
		{

			var message = new MimeMessage();
			message.From.Add(new MailboxAddress("Malek sarker", "limon14203165@gmail.com"));
			message.To.Add(new MailboxAddress("limon", "limon14203165@gmail.com"));
			message.Subject = "How you doin'?";
			//plain text
			/*
			message.Body = new TextPart("plain")
			{
				Text = @"Hey Chandler,
					I just wanted to let you know that Monica and I were going to go play some paintball, you in?-- Joey"
			}; 
			plain text body*/
			var builder = new BodyBuilder();

			// Set the plain-text version of the message text
			builder.TextBody = @"Hey Limon,
			What are you up to this weekend? Monica is throwing one of her parties on
			Saturday and I was hoping you could make it.

			Will you be my +1?-- Joey";
			builder.Attachments.Add(@"C:\Users\limon\Desktop\1.webp");

			// Now we just need to set the message body and we're done
			message.Body = builder.ToMessageBody();

			using (var client = new SmtpClient())
			{
				client.Connect("smtp.gmail.com", 587, false);

				// Note: only needed if the SMTP server requires authentication
				client.Authenticate("limon14203165@gmail.com", "liibd786");

				client.Send(message);
				client.Disconnect(true);
			}

			//pop 3 //
			/*
			using (var client = new Pop3Client())
			{
				client.Connect("pop.gmail.com", 995, false);

				client.Authenticate("limon14203165@gmail.com", "liibd786");

				for (int i = 0; i < client.Count; i++)
				{
					var getmessage = client.GetMessage(i);
					Console.WriteLine("Subject: {0}", getmessage.Subject);
				}

				client.Disconnect(true);
			}
			*/

			//Imap client///Impap client
			var messageList = new List<ReceiveMail>();
			using (var client = new ImapClient())
			{
				client.Connect("imap.gmail.com", 993, true);

				client.Authenticate("limon14203165@gmail.com", "liibd786");

				// The Inbox folder is always available on all IMAP servers...
				var inbox = client.Inbox;
				inbox.Open(FolderAccess.ReadWrite);

				Console.WriteLine("Total messages: {0}", inbox.Count);
				Console.WriteLine("Recent messages: {0}", inbox.Recent);

				//fetch attachment//	 

				//string saveAttachmentPath = @"C:\Users\mimeKitAttachments\";
				//IList<UniqueId> uids = client.Inbox.Search(SearchQuery.All);
				//if (inbox != null)
				//{
				//	foreach (var x in uids)
				//	{
				//		var message = inbox.GetMessage(x);

				//		foreach (MimeEntity attachment in message.BodyParts)
				//		{
				//			attachment.WriteTo(saveAttachmentPath);
				//		}
				//	}
				//}
				//
				//fetch inbox
				for (int i = 0; i < (inbox.Count > 5 ? 5 : inbox.Count); i++)
				{
					var inmessage = inbox.GetMessage(i);
					ReceiveMail receiveMail = new ReceiveMail();
					receiveMail.messageId = inmessage.MessageId;
					receiveMail.messageSubject = inmessage.Subject;
					receiveMail.messageFrom = inmessage.TextBody;
					//var replacetext = Regex.Replace(inmessage.Body, @"(<.*?>)|({.*})|(.*[}\)--{;]\s?\r\n)|(\r?\n@.*)|(.*[}{;]$)", String.Empty).Trim;
					Console.WriteLine("Name: {0}\t,From:{1}\t,UID:{2},Body:{3}", inmessage.From[0].Name, inmessage.Date, inmessage.MessageId,inmessage.TextBody);
					messageList.Add(receiveMail);
				}
				foreach (var item in messageList)
				{
					Console.WriteLine("Subject: {0},From:{1},UID:{2} ", item.messageId,item.messageSubject,item.messageFrom);
				}
				///fetch draft email									  
				if ((client.Capabilities & (ImapCapabilities.SpecialUse | ImapCapabilities.XList)) != 0)
				{
					var drafts = client.GetFolder(SpecialFolder.Drafts);
					Console.WriteLine("Draft messages: {0}", drafts.Count);
					Console.WriteLine("Recent messages: {0}", drafts.Recent);

				}
				else
				{
					// maybe check the user's preferences for the Drafts folder?
				}
				 //fetch folder name
				var personal = client.GetFolder(client.PersonalNamespaces[0]);
				foreach (var folder in personal.GetSubfolders(true))
					Console.WriteLine("[folder] {0}", folder.Name);

				client.Disconnect(true);
			}
			//Imap client 
		}
	}
}

