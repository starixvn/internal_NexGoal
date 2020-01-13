import React, { Component, Fragment } from 'react';
import isEqual from 'react-fast-compare';

import getResource from '../../../resourceHelper';
import { getRoleClassName } from '../../../common-functions';

export default class Node extends Component {
    shouldComponentUpdate(nextProps) {
        return !isEqual(this.props, nextProps);
    }

    handleClick = () => {
        this.props.onClick(this.props.data.performanceRecordId);
    }

    render() {
        const { data, position } = this.props;

        return (
            <Fragment>
                <p className={`timeline-grade ${getRoleClassName(data.roleId)}`} style={{ right: position + 14 }} onClick={this.handleClick} >
                    {data.grade ? data.grade.name : getResource('N/A')}
                </p>
                <div className="timeline-info" style={{ right: position }} onClick={this.handleClick}>
                    <p className={`name ${getRoleClassName(data.roleId)}`}>{data.creator.nickName}</p>
                </div>
            </Fragment>
        );
    }
}