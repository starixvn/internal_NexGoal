import React, { Component, Fragment } from 'react';

import Row from './Row';

function THead(props) {
    return (
        <thead>{props.children}</thead>
    );
}

function TBody(props) {
    return (
        <tbody>{props.children}</tbody>
    );
}

class Table extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        const isHost = this.props.isHost;
        const flowTemplates = this.props.flowTemplates;

        return (
            <Fragment>
                <table className="table table-bordered">
                    <THead>
                        {isHost &&
                            <tr className="header-control">
                                <th colSpan="9">
                                    <a className="pull-left btn btn-primary">
                                        <i className="fa fa-plus"></i>Create Evaluation Flow
                                    </a>
                                </th>
                            </tr>
                        }
                        <tr>
                            <th>#</th>
                            <th>Name</th>
                            <th>Type</th>
                            <th>Active</th>
                            <th>Description</th>
                            <th>Action</th>
                        </tr>
                    </THead>
                    <TBody>
                        {flowTemplates.length === 0 &&
                            <tr>
                                <td className="empty-result" colSpan="6">No information available!</td>
                            </tr>
                        }
                        {flowTemplates.length > 0 &&
                            flowTemplates.map((flowTemplate, index) => {
                                return (
                                    <Row key={'flowTemplate-' + flowTemplate.flowTemplateId}
                                        rowIndex={index + 1}
                                        flowTemplate={flowTemplate}
                                        isHost={isHost} />)
                            })
                        }
                    </TBody>
                </table>
            </Fragment>
        );
    }
}

export default Table;