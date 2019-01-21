import React from 'react';

export default function Marquee(props) {
    return (
        <div className="warning marquee" onClick={props.onClick} >
            <span className={`annoucement-${props.messages.length}`}>
                {
                    props.messages.map((annoucement, index) =>
                        <span key={`annoucement${index}`} className="announcement-item">
                            {annoucement}&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        </span>
                    )
                }
            </span>
        </div>
    );
}