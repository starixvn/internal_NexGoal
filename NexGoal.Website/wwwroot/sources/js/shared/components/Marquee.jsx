import React from 'react';

export default function Marquee(props) {
    return ( 
        <div className="warning marquee" onClick={props.onClick} >
            <span className={`annoucement-${props.messages.length}`}>
                {props.messages.map(e => <span className="announcement-item">{e}&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>)}
            </span>
        </div>
    );
}