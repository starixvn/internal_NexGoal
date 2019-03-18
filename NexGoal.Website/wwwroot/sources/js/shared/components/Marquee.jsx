import React from 'react';

export default function Marquee(props) {
    var duration = props.messages.reduce((total, current) => total + current.length, 0) / 5;

    var style = {
        '-webkit-animation': `marquee ${duration}s linear infinite`,
        '-khtml-animation': `marquee ${duration}s linear infinite`,
        '-moz-animation': `marquee ${duration}s linear infinite`,
        '-ms-animation': `marquee ${duration}s linear infinite`,
        '-o-animation': `marquee ${duration}s linear infinite`,
        'animation': `marquee ${duration}s linear infinite`
    }

    return (
        <div className="warning marquee" onClick={props.onClick} >
            <span className="annoucement" style={style}>
                {
                    props.messages.map((annoucement, index) =>
                        <span key={`annoucement${index}`} className="announcement-item">
                            {annoucement}
                        </span>
                    )
                }
            </span>
        </div>
    );
}