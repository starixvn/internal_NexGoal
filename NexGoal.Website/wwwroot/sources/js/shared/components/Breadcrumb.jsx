import React from 'react';

const Breadcrumb = props => (
    <div className="col-sm-12 sub-header">
        <ul className="row breadcrumb">
            {props.children}
        </ul>
    </div>
);

export default Breadcrumb;