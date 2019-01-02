import React from 'react';

export default function Marquee(props) {
    return (
        <div className="warning marquee" onClick={props.onClick} >
            <span>
                {props.messages.join(' ')}
            </span>
        </div>
    );
}