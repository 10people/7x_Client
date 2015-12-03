using UnityEngine;

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

public class QXBuffer
{
	// protocol str
	public int m_protocol_index = 0;

	private static List<QXBuffer> m_buffer_pool = new List<QXBuffer>();
	
	private MemoryStream m_stream;
	private BinaryWriter m_writer;
	private BinaryReader m_reader;
	
	private int m_counter = 0;
	private int m_data_size = 0;
	private bool m_writing = false;
	private bool m_in_pool = false;
	
	QXBuffer (){
		m_stream = new MemoryStream();
		m_writer = new BinaryWriter( m_stream );
		m_reader = new BinaryReader( m_stream );
	}
	
	~QXBuffer (){
		m_stream.Dispose();
	}

	public int size{
		get{
			return m_writing ? (int)m_stream.Position : m_data_size - (int)m_stream.Position;
		}
	}
	
	public int position { 
		get{ 
			return (int)m_stream.Position; 
		} 

		set{
			m_stream.Seek(value, SeekOrigin.Begin); 
		} 
	}
	
	public MemoryStream stream { 
		get{
			return m_stream; 
		} 
	}
	
	public byte[] buffer{
		get{
			return m_stream.GetBuffer(); 
		} 
	}

	public byte[] m_protocol_message{
		get{
			//return m_stream.ToArray();

			return buffer;
		}
	}

	public static int GetAvailableBufferCount(){
		return m_buffer_pool.Count;
	}
	
	public static QXBuffer Create(){
		QXBuffer t_bffer = null;
		
		if( m_buffer_pool.Count == 0 ){
			t_bffer = new QXBuffer();
		}
		else{
			lock( m_buffer_pool ){
				if( m_buffer_pool.Count != 0 ){
					t_bffer = m_buffer_pool[ 0 ];
					
					m_buffer_pool.RemoveAt( 0 );
					
					t_bffer.m_in_pool = false;
				}
				else{
					t_bffer = new QXBuffer();
				}
			}
		}

		t_bffer.m_counter = 1;

		t_bffer.ResetCreateTimeTag();

		return t_bffer;
	}

	public bool Recycle(){
		if( !m_in_pool && ( MarkAsUnused() ) ){
			m_in_pool = true;
			
			lock( m_buffer_pool ){
				Clear();

				m_buffer_pool.Add( this );
			}
			return true;
		}
		return false;
	}

	public void MarkAsUsed(){
		Interlocked.Increment( ref m_counter ); 
	}
	
	public bool MarkAsUnused(){
		if( Interlocked.Decrement( ref m_counter ) > 0 ){
			return false;
		}

		m_data_size = 0;

		m_stream.Seek( 0, SeekOrigin.Begin );

		m_writing = true;

		return true;
	}
	
	public void Clear(){
		m_counter = 0;

		m_data_size = 0;

		if ( m_stream.Capacity > 1024 ){
			m_stream.SetLength(256);
		}

		m_stream.Seek( 0, SeekOrigin.Begin );

		m_writing = true;
	}

	public void Dispose (){
		m_stream.Dispose();
	}
	
	public BinaryWriter BeginWriting( bool append ){
		if( !append || !m_writing ){
			m_stream.Seek(0, SeekOrigin.Begin);

			m_data_size = 0;
		}

		m_writing = true;

		return m_writer;
	}
	
	public BinaryWriter BeginWriting( int startOffset ){
		m_stream.Seek(startOffset, SeekOrigin.Begin);

		m_writing = true;

		return m_writer;
	}
	
	public int EndWriting(){
		if( m_writing ){
			m_data_size = position;

			m_stream.Seek( 0, SeekOrigin.Begin );

			m_writing = false;
		}

		return m_data_size;
	}
	
	public BinaryReader BeginReading(){
		if( m_writing ){
			m_writing = false;

			m_data_size = (int)m_stream.Position;

			m_stream.Seek( 0, SeekOrigin.Begin );
		}

		return m_reader;
	}
	
	public BinaryReader BeginReading( int startOffset ){
		if( m_writing ){
			m_writing = false;

			m_data_size = (int)m_stream.Position;
		}

		m_stream.Seek( startOffset, SeekOrigin.Begin );

		return m_reader;
	}

	#region Emulate Network Latency

	private bool m_is_new_created = false;

	private float m_create_time_tag		= 0.0f;

	private long m_create_time_long		= 0;

	private void ResetCreateTimeTag(){
		m_is_new_created = true;

		m_create_time_long = TimeHelper.GetCurrentTimeMillis();
	}

	public float GetTimeAfterCreate(){
		if( m_is_new_created ){
			m_create_time_tag = Time.realtimeSinceStartup;

			m_is_new_created = false;
		}

		return Time.realtimeSinceStartup - m_create_time_tag;
	}

	public float GetCreateTimeTag(){
		return m_create_time_tag;
	}

	public long GetTimeMillis(){
		return m_create_time_long;
	}

	#endregion



	#region Network Waiting

	private bool m_is_sending_wait = true;

	private bool m_is_receiving_wait = false;

	private void ResetWaitingFlag(){
		m_is_sending_wait = true;

		m_is_receiving_wait = false;
	}

	public bool IsSendingWait(){
		return m_is_sending_wait;
	}

	public bool IsReceivingWait(){
		return m_is_receiving_wait;
	}

	public void SetSendingWait( bool p_sending_wait ){
		m_is_sending_wait = p_sending_wait;
	}

	public void SetReceivingWait( bool p_receiving_wait ){
		m_is_receiving_wait = p_receiving_wait;
	}

	#endregion
}