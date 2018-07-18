import React, { Component, Fragment } from 'react';

import Row from './Row';
import CreateEOY from './Create';
import DeleteEOY from './Delete';

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

        this.state = {
            showCreateModal: false,
            showDeleteModal: false,
            eoySelected: null
        };
    }

    showCreateModal() {
        this.setState({ showCreateModal: true });
    }

    closeCreateModal() {
        this.setState({ showCreateModal: false });
    }

    showDeleteModal(eoySelected) {
        this.setState({
            showDeleteModal: true,
            eoySelected: eoySelected
        });
    }

    closeDeleteModal() {
        this.setState({
            showDeleteModal: false,
            eoySelected: null
        });
    }

    render() {
        const isHost = this.props.isHost;
        const eoys = this.props.eoys;

        return (
            <Fragment>
                <table className="table table-bordered">
                    <THead>
                        {isHost &&
                            <tr className="header-control">
                                <th colSpan="9">
                                    <a className="pull-left btn btn-primary" onClick={this.showCreateModal.bind(this)}>
                                        <i className="fa fa-plus"></i>Create Nomination Period
                                </a>
                                </th>
                            </tr>
                        }
                        <tr>
                            <th>#</th>
                            <th>Year</th>
                            <th>Start Date For Nominator</th>
                            <th>End Date For Nominator</th>
                            <th>Remind Date For Nominator</th>
                            <th>End Date For Committee</th>
                            <th>Status</th>
                            <th>Action</th>
                        </tr>
                    </THead>
                    <TBody>
                        {eoys.length === 0 &&
                            <tr>
                                <td className="empty-result" colSpan="9">No information available!</td>
                            </tr>
                        }
                        {eoys.length > 0 &&
                            eoys.map((eoy, index) => {
                                return (
                                    <Row key={'eoy-' + eoy.eoqyId}
                                        rowIndex={index + 1}
                                        eoy={eoy}
                                        isHost={isHost}
                                        showDeleteModal={this.showDeleteModal.bind(this)} />)
                            })
                        }
                    </TBody>
                </table>

                {
                    this.state.showCreateModal &&
                    <CreateEOY
                        showModal={this.state.showCreateModal}
                        closeModal={this.closeCreateModal.bind(this)}
                        handleFilterSubmit={this.props.handleFilterSubmit} />
                }
                {
                    this.state.showDeleteModal &&
                    <DeleteEOY
                        eoySelected={this.state.eoySelected}
                        showModal={this.state.showDeleteModal}
                        closeModal={this.closeDeleteModal.bind(this)}
                        handleFilterSubmit={this.props.handleFilterSubmit} />
                }
            </Fragment>
        );
    }
}

export default Table;